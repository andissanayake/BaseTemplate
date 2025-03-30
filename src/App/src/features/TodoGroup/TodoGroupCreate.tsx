import React from "react";
import { Modal, Form, Input, notification } from "antd";
import { useTodoGroupStore } from "./todoGroupStore";

interface TodoGroupModalProps {
  visible: boolean;
  onClose: () => void;
}

const TodoGroupCreate: React.FC<TodoGroupModalProps> = ({
  visible,
  onClose,
}) => {
  const [form] = Form.useForm();
  const { createTodoGroup } = useTodoGroupStore();

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      await createTodoGroup(values);
      notification.success({ message: "Operation successful!" });
      onClose();
      form.resetFields();
    });
  };

  return (
    <Modal
      title={"Add New Todo Group"}
      open={visible}
      onCancel={onClose}
      onOk={handleSaveTodoGroup}
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Todo Group Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo group name!" },
          ]}
        >
          <Input placeholder="Enter todo group name" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoGroupCreate;
