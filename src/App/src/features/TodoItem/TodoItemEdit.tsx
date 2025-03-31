/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";
import { useTodoItemStore } from "./todoItemStore";
import { Form, Input, Modal, notification, DatePicker, Select } from "antd";
import { TodoItemService } from "./todoItemService";
import dayjs from "dayjs";

interface TodoItemModalProps {
  visible: boolean;
  onClose: () => void;
}

export const TodoItemEdit: React.FC<TodoItemModalProps> = ({
  visible,
  onClose,
}) => {
  const { editTodoItem, setTodoItemEdit } = useTodoItemStore();
  const [form] = Form.useForm();

  useEffect(() => {
    if (editTodoItem?.id) {
      form.setFieldsValue({
        ...editTodoItem,
        reminder: editTodoItem.reminder
          ? dayjs(editTodoItem.reminder)
          : undefined,
      });
    }
  }, [editTodoItem, form]);

  const handleSaveTodoItem = () => {
    form.validateFields().then(async (values) => {
      const updatedItem = {
        ...values,
        id: editTodoItem?.id,
        reminder: values.reminder?.toISOString() ?? null,
      };

      try {
        await TodoItemService.updateTodoItem(updatedItem);
        notification.success({ message: "Operation successful!" });
        setTodoItemEdit(null);
        onClose();
      } catch (error: any) {
        console.error("Error updating todo item:", error);
        notification.error({ message: "Failed to update todo item!" });
      }
    });
  };

  return (
    <Modal
      title={"Edit Todo Item"}
      open={visible}
      onCancel={() => {
        setTodoItemEdit(null);
        onClose();
      }}
      onOk={handleSaveTodoItem}
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Title"
          name="title"
          rules={[{ required: true, message: "Please enter the title!" }]}
        >
          <Input placeholder="Enter todo item title" />
        </Form.Item>

        <Form.Item label="Note" name="note">
          <Input.TextArea rows={3} placeholder="Optional note..." />
        </Form.Item>

        <Form.Item label="Reminder" name="reminder">
          <DatePicker
            style={{ width: "100%" }}
            showTime
            format="YYYY-MM-DD HH:mm"
          />
        </Form.Item>

        <Form.Item label="Priority Level" name="priorityLevel">
          <Select options={TodoItemService.getPriorityLevels()} />
        </Form.Item>
      </Form>
    </Modal>
  );
};
