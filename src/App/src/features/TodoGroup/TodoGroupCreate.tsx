import React from "react";
import { Modal, Form, Input, notification } from "antd";
import { TodoGroupService } from "./todoGroupService";

interface TodoGroupModalProps {
  visible: boolean;
  onClose: () => void;
}

const TodoGroupCreate: React.FC<TodoGroupModalProps> = ({
  visible,
  onClose,
}) => {
  const [form] = Form.useForm();

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      const data = await TodoGroupService.createTodoGroup(values);
      console.log("Todo group created:", data);
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
